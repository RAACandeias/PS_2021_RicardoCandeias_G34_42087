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
expand(P, l(N, F/G), Bound, Tree1, Solved, Solution) :-
	F => Bound,
	(
		bagof(
			M/C, (s(N, M, C), \+member(M,P)), Succ
		),
		!,
		succlist(G, Succ, Ts),
		bestf(Ts, F1),
		expand(P,t(N, F1/G, Ts), Bound, Tree1, Solved, Sol)
		;
		Solved = never
	).

%case 3: non-leaf, f-value < Bound, expand most promising sub-tree
expand(P, t(N, F/G, [T|Ts]), Bound, Tree1, Solved, Solution) :- 
	F => Bound,
	besft(Ts, BF), min(Bound, BF, Bound1),
	expand([N|P], T, Bound1, T1, Solved1, Solution),
	continue(P, t(N, F/G, [T1|Ts]), Bound, Tree1, Solved1, Solved, Solution).
	
%case 4: non-leaf with empty subtrees
expand(_, t(_, _, [])_, _, never, _) :- !.

%case 5: value > Bound
expand(_, Tree, Bound, Tree, no, _) :- f(Tree, F), F > Bound)

%------- continue(path, Tree, Bound, NewTree, SubtreeSolved, TreeSolved, Solution) --------|

continue(_, _, _, _, yes, yes, Solution).

continue(P, t(N, F/G, [T1|Ts]), Bound, Tree1, no, Solved, Solution) :-
	insert(T1, Ts, NTs),
	bestF(NTs, F1),
	expand(P, t(N, F1/G, NTs), Bound,Tree1,Solved,Solution).

% não percebi bem esta continuação

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
	insert( l(N, F/G), Ts1, Ts)).
	
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

bestF([T | _], F) :- %best value of list of trees 
	f(T, F).         %garanteed by insert
	
bestF([], 9999). %no tree bad F value
















