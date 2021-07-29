


:- dynamic(obstacle/1).
:- dynamic(goal/1). 

walls([0/0,0/1,2/0,2/1,1/3,3/3,5/5,8/8,6/9]).

xMaxCoord(9).
yMaxCoord(9).

adjacent(X1/Y1, X2/Y1) :- 
	xMaxCoord(Xmax),
	((X2 is X1 + 1,
	X1 < Xmax)  
	; 
	(X2 is X1 - 1,
	X1 > 0)).
	
adjacent(X1/Y1, X1/Y2) :- 
	yMaxCoord(Ymax),
	((Y2 is Y1 + 1,
	Y1 < Ymax)  
	; 
	(Y2 is Y1 - 1,
	Y1 > 0)).
	
s(A, B, 1) :- adjacent(A, B), 
	walls(Set),
	\+ member(B, Set),
	\+ obstacle(B).
	
h(X1/Y1, R) :-
	goal(X2/Y2),
	R is sqrt((X2 - X1)**2 + (Y2 - Y1)**2).

:- ensure_loaded(['Astar.pl']).




