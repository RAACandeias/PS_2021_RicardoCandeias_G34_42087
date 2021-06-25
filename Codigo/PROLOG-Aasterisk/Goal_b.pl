:- current_predicate(goal/1), retract( goal(_) ).
:- current_predicate(h/2),retract( h(_,_) ).
:- \+current_predicate(goal/1), dynamic( goal/1).
:- \+current_predicate(h/2), dynamic( h/2).

goal(b).

h(a, 5).
h(b, 0).
h(c, 5).
h(d, 11.5).
h(e, 4.5).
h(f, 13).
h(g, 9).
h(i, 14.5).
h(h, 13).
h(j, 21).
h(k, 16).
h(l, 20).
h(m, 24).
h(n, 25).
h(o, 28).
h(p, 30.5).
h(s, 35.5).
h(q, 35.5).
h(r, 37.5).
h(t, 41.5).
h(u, 43).
