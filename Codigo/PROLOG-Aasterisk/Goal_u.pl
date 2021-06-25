:- current_predicate(goal/1), retract( goal(_) ).
:- current_predicate(h/2),retract( h(_,_) ).
:- \+current_predicate(goal/1), dynamic( goal/1).
:- \+current_predicate(h/2), dynamic( h/2).

goal(u).

h(a, 47).
h(b, 42.2).
h(c, 41).
h(d, 44.5).
h(e, 36.5).
h(f, 37).
h(g, 32).
h(i, 29).
h(h, 29.5).
h(j, 24).
h(k, 26).
h(l, 20.5).
h(m, 18).
h(n, 19.5).
h(o, 13.5).
h(p, 10.5).
h(s, 7).
h(q, 4.5).
h(r, 6).
h(t, 10.5).
h(u, 0).
