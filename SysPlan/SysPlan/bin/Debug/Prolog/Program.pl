


:- dynamic(obstacle/1).
:- dynamic(goal/1). 

walls([0/0,3/0,6/0,9/0,
	   0/1,3/1,6/1,9/1,
	   0/2,3/2,6/2,9/2,
	   1/3,8/3,
	   4/4,5/4,
	   0/5,3/5,6/5,9/5,
	   2/7,7/7,
	   0/9,1/9,2/9,7/9,8/9,9/9]).

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




