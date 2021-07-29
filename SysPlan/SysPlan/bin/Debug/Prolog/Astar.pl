
/*-------------------------------------------------------------------*/

bestfirst(Start, Solution) :- 
	expand([], l(Start, 0/0), 9999, _, yes, Solution).

expand(P, l(N, _),_, _, yes, [N|P]) :- goal(N).

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

expand( P, t(N, F/G, [T | Ts]), Bound, Tree1, Solved, Sol) :-
	F =< Bound,
	bestf(Ts, BF), Bound1 = min( Bound, BF), 
	expand([N | P], T, Bound1, T1, Solved1, Sol),
	continue(P, t(N, F/G, [T1 | Ts]), Bound, Tree1, Solved1, Solved, Sol).
	
expand( _, t(_, _, []), _, _, never, _) :- !.

expand(_, Tree, Bound, Tree, no, _) :- 
	f(Tree, F), F > Bound.

continue(_, _, _, _, yes, yes, _).

continue(P, t(N, _/G, [T1|Ts]), Bound, Tree1, no, Solved, Solution) :-
	insert(T1, Ts, NTs),
	bestf(NTs, F1),
	expand(P, t(N, F1/G, NTs), Bound,Tree1,Solved,Solution).

continue( P, t(N, _/G, [_ | Ts]), Bound, Tree1, never, Solved, Sol) :-
	bestf( Ts, F1),
	expand( P, t(N, F1/G, Ts), Bound, Tree1, Solved, Sol).

succlist(_, [], []).

succlist(G0, [N/C | NCs], Ts) :-
	G is G0 + C, %calculo do custo incluindo o successor
	h(N, H),     %heuristica para o successor
	F is G + H,
	succlist(G0, NCs, Ts1),
	insert( l(N, F/G), Ts1, Ts).

insert(T, Ts, [T | Ts]) :- 
	f(T, F),
	bestf(Ts, F1),
	F =< F1, !.

insert(T, [T1 | Ts], [T1 | Ts1]) :- 
	insert(T, Ts, Ts1).

f(l(_, F/_), F).    %F-value of leaf

f(t(_, F/_, _), F). %f-value of tree

bestf([T | _], F) :- %best value of list of trees 
	f(T, F).         %garanteed by insert
	
bestf([], 9999). %no tree bad F value

















