:- dynamic( obstacle/1).


program :- 
	write('\33\[2J'),
    nl,write(' 1.Load Map '),nl,
    write(' 2.Load A*  '),nl,
    write(' 3.Load Goal: u '),nl,
	write(' 4.Load Goal: b '),nl,
	write(' 5.Place Obstacle '),nl,
	write(' 6.Remove Obstacle '),nl,
	write(' 7.Present Obstacles '),nl,
	write(' 8.Find Path '),nl,
	write(' 9.Clear window '),nl,
	write(' 0.End Program '),nl,nl,
    write('enter your choice:'),nl,
    read(Option), Option >= 0, Option =< 9,
    exe(Option).
	
program_next :- 
	nl,write('enter your choice:'),nl,
    read(Option), Option >= 0, Option =< 9,
    exe(Option).
	
exe(1) :- 
	ensure_loaded(['Map.pl']),write('Map loaded!'),nl, 
	program_next.
exe(2) :- 
	ensure_loaded(['A(asterisk).pl']),write('A* loaded!'),nl,    
	program_next.
exe(3) :- 
	consult(['Goal_u.pl']),write('Goal u set!'),nl,    
	program_next.
exe(4) :- 
	consult(['Goal_b.pl']),write('Goal b set!'),nl,    
	program_next.
exe(5) :- 
	write('Type obstacle node: '),nl,read(Node),char_type(Node,lower),
	asserta(obstacle(Node)),nl,
	program_next.
exe(6) :- 
	write('Type obstacle node: '),nl,read(Node),char_type(Node,lower),
	retract(obstacle(Node)),nl,
	program_next.
exe(7) :- 
	write('Current Obstacles: '),nl,
	findall(X, obstacle(X), Obstacles),write(Obstacles),nl,
	program_next.
exe(8) :- 
	write('Type start node: '),nl,read(Start),char_type(Start,lower),
	bestfirst(Start, Path),
	write('Calculated best Path: '),nl,
	write(Path),nl,
	program_next.	
exe(9) :- program.	
exe(0) :- abort.
