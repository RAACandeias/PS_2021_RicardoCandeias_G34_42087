%s( Node, Node1, Cost) - This is true if there is an arc, costing Cost, between Node and Node1, 
% in the state space.

s( s, a, 2).

s( s, e, 2).

% left subtree
s( a, b, 2).

s( b, c, 2).

s( c, d, 3).

s( d, t, 3).

% right subtree
s( e, f, 5).

s( f, g, 2).

s( g, t, 2).

% goal(Node) - is true if Node is a goal node in the state space.
%
goal(t).

% h( Node, H) - H is a heuristic estimate of the cost of a 
% cheapest path from Node to a goal node.

% left subtree
h(a, 5).

h(b, 4).

h(c, 4).

h(d, 3).

% right subtree
h(e, 7).

h(f, 4).

h(g, 2).

% ADDED
h(t, 0).

%
%	data structure example start:
%	t( startNode, Cost1/0, [l(SuccOfStart1, Cost1/distSum1), l(SuccOfStart2, Cost2/distSum2)] )
%	if Cost1 < Cost2 && Cost2 = distSum + distEuclid(heuristic from SuccOfStart2 to end node)
%	
%	data structure example expanded:
%	t( startNode, Cost1/0, [l(SuccOfStart1, Cost1/distSum1), t(SuccOfStart2, Cost5/distSum3, t(succSuccOfStart2, Cost5/distSum4, [l(succSuccSuccOfstart2, Cost5/distSum5)]))] )
%	here we see process switch because Cost1 < Cost5 && Cost5 = distSum5 + distEuclid(heuristic from succSuccSuccOfstart2 to end node)
%
%----------- Tree = t(N, F/G, [T | Ts]) ---------------------------------------|
%
%----------- expand(P, Tree, Bound, Tree1, Solved, Solution) ------------------|
%
%	expands a current (sub)tree 'Tree' as long as the f-value (Custo: distancia percurrida + distancia euclidiana)
%	of this tree remains less or equal to 'Bound'
%		(input) P        - path between the start node and 'Tree'
%		(input) Tree     - Current search (sub)tree
%		(input) Bound    - f-limit for expansion of 'Tree'
%		Tree1    - 'tree' expanded within Bound; f-value of 'tree1' is greater than 'bound' (unless goal node found)
%		Solved   - indicator ('yes, 'no' or 'never') of whether a goal as been found during expansion
%		Solution - The solution path from start node throught tree1 to goal node, within 'bound'	
%
%	Produces 3 kinds of results:
%		(1) Solved = yes
%			Solution = a solution path found by expanding Tree within Bound
%			Tree1 = uninstantiated
%			
%		(2) Solved = no
%			Solution = uninstantiated
%			Tree1 = Tree expanded so thatits f-value exceeds Bound
%			
%		(3) Solved = never              (happens when f-value of Tree <= Bound and there is no successor or will cause cycle)
%			Solution = uninstantiated
%			Tree1 = uninstantiated

bestfirst(Start, Solution) :- 
	expand([], l(Start, 0/0), 9999, _, yes, Solution).

%leaf-node -> single node tree l(N, F/G)
	
% case 1: leaf-node, Construct solution
expand(P, l(N, _),_, _, yes, [N|P]) :- goal(N).

%case 2: leaf-node, f-value less than Bound, expand within bound
expand(P, l(N, F/G), Bound, Tree1, Solved, Sol) :-
	F =< Bound,
	( 
		bagof( M/C, ( s(N, M, C), \+ member(M, P)), Succ),
		!,                      % Node N has successors
		succlist( G, Succ, Ts), % Make subtrees Ts
		bestf( Ts, F1),         % f-value of best successor
		expand(P, t(N, F1/G, Ts), Bound, Tree1, Solved, Sol)
		;
		Solved = never           % N has no successors - dead end
	).

%case 3: non-leaf, f-value < Bound, expand most promising sub-tree
expand( P, t(N, F/G, [T | Ts]), Bound, Tree1, Solved, Sol) :-
	F =< Bound,
	bestf(Ts, BF), Bound1 = min( Bound, BF), % min( Bound, BF, Bound1) does not work in SWI-Prolog
	expand([N | P], T, Bound1, T1, Solved1, Sol),
	continue(P, t(N, F/G, [T1 | Ts]), Bound, Tree1, Solved1, Solved, Sol).
	
%case 4: non-leaf with empty subtrees
expand( _, t(_, _, []), _, _, never, _) :- !.

%case 5: value > Bound
expand(_, Tree, Bound, Tree, no, _) :- 
	f(Tree, F), F > Bound.

%------- continue(path, Tree, Bound, NewTree, SubtreeSolved, TreeSolved, Solution) --------|

continue(_, _, _, _, yes, yes, Solution).

continue(P, t(N, F/G, [T1|Ts]), Bound, Tree1, no, Solved, Solution) :-
	insert(T1, Ts, NTs),
	bestf(NTs, F1),
	expand(P, t(N, F1/G, NTs), Bound,Tree1,Solved,Solution).

continue( P, t(N, F/G, [_ | Ts]), Bound, Tree1, never, Solved, Sol) :-
	bestf( Ts, F1),
	expand( P, t(N, F1/G, Ts), Bound, Tree1, Solved, Sol).

%-------- succlist(G0, [Node1/Cost1, ...], [l(BestNode, BestF/G, ...)]) make list of search leaves ordered by their f-values ------------|
% G0 custo somado até aqui
% Cost1 custo para o proximo succ

succlist(_, [], []).

succlist(G0, [N/C | NCs], Ts) :-
	G is G0 + C, %calculo do custo incluindo o succ
	h(N, H),     %heuristica para o succ
	F is G + H,
	succlist(G0, NCs, Ts1),
	insert( l(N, F/G), Ts1, Ts).
	
% Insert T into list of Trees Ts preserving order with respect to f-values
% Menor para Maior

insert(T, Ts, [T | Ts]) :- 
	f(T, F),
	bestf(Ts, F1),
	F =< F1, !.

insert(T, [T1 | Ts], [T1 | Ts1]) :- 
	insert(T, Ts, Ts1).
	
%Extract the Best F-value

f(l(_, F/_), F).    %F-value of leaf

f(t(_, F/_, _), F). %f-value of tree

bestf([T | _], F) :- %best value of list of trees 
	f(T, F).         %garanteed by insert
	
bestf([], 9999). %no tree bad F value
















