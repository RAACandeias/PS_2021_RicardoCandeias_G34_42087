%s( Node, Node1, Cost) - This is true if there is an arc, costing Cost, between Node and Node1, 
% in the state space.%

s( a, b, 5).

s( a, c, 4).

s( a, d, 7).


s( b, a, 5).

s( c, a, 4).

s( d, a, 7).


%-------------%

s( b, e, 4.5).

s( b, i, 14.5).


s( e, b, 4.5).

s( i, b, 14.5).

%-------------%

s( c, e, 6).

s( c, g, 6).

s( c, f, 6.5).


s( e, c, 6).

s( g, c, 6).

s( f, c, 6.5).

%-------------%

s( d, f, 4.5).


s( f, d, 4.5).

%-------------%

s( f, i, 6).


s( i, f, 6).

%-------------%

s(e,h,6).


s(h,e,6).

%-------------%

s(g,i,5.5).

s(g,h,6).

s(g,l,7).


s(i,g,5.5).

s(h,g,6).

s(l,g,7).

%-------------%

s(i,j,4.5).


s(j,i,4.5).

%-------------%

s(h,k,2).

s(h,j,15).


s(k,h,2).

s(j,h,15).

%-------------%

s(j,m,5).


s(m,j,5).

%-------------%

s(k,n,7).

s(k,l,5).


s(n,k,7).

s(l,k,5).

%-------------%

s(l,m,4.5).

s(l,q,16.5).

s(l,o,7).


s(m,l,4.5).

s(q,l,16.5).

s(o,l,7).

%-------------%

s(m,p,7).


s(p,m,7).

%-------------%

s(n,q,8.75).

s(n,o,5).


s(q,n,8.75).

s(o,n,5).

%-------------%

s(o,r,7.5).

s(o,p,3.5).


s(r,o,7.5).

s(p,o,3.5).

%-------------%

s(p,u,10.5).

s(p,s,4.5).


s(u,p,10.5).

s(s,p,4.5).

%-------------%

s(s,u,7).


s(u,s,7).

%-------------%

s(q,r,6).

s(q,t,4.5).


s(r,q,6).

s(t,q,4.5).

%-------------%

s(r,u,6).


s(u,r,6).

%-------------%

s(t,u,10.5).


s(u,t,10.5).

%-------------%

%s(X,Y,C) :- s(Y,X,C).