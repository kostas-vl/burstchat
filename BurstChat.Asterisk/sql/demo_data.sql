insert into ps_aors (id, max_contacts) values ('aor5000', 5);
insert into ps_aors (id, max_contacts) values ('aor2000', 5);

insert into ps_auths (id, auth_type, password, username) values ('auth5000', 'userpass', '5000@pass1', '5000');
insert into ps_auths (id, auth_type, password, username) values ('auth2000', 'userpass', '2000@pass1', '2000');

insert into ps_endpoints
    (id, transport, aors, auth, dtls_auto_generate_cert, webrtc, context, disallow, allow)
values
    ('5000', 'transport-ws', 'aor5000', 'auth5000', 'yes', 'yes', 'helloworld', 'all', 'alaw,ulaw,opus');

insert into ps_endpoints
    (id, transport, aors, auth, dtls_auto_generate_cert, webrtc, context, disallow, allow)
values
    ('2000', 'transport-ws', 'aor2000', 'auth2000', 'yes', 'yes', 'helloworld', 'all', 'alaw,ulaw,opus');
