CREATE SCHEMA IF NOT EXISTS {{cookiecutter.database_name}};

{% if cookiecutter.include_audit == 'yes' %}
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TABLE IF NOT EXISTS {{cookiecutter.database_name}}.audit_event
(
	event_id     SERIAL PRIMARY KEY,
	data         jsonb,
	last_updated TIMESTAMP WITH TIME ZONE,
	event_type   TEXT NOT NULL
);

/* PGP: Symmetric */
CREATE OR REPLACE FUNCTION {{cookiecutter.database_name}}.db_sym_encrypt(t text, k text) RETURNS bytea AS $function$
BEGIN
   RETURN pgp_sym_encrypt(t, k);
END;
$function$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION {{cookiecutter.database_name}}.db_sym_decrypt(t bytea, k text) RETURNS text AS $function$
BEGIN
   RETURN pgp_sym_decrypt(t,k);
END;
$function$ LANGUAGE plpgsql;	
{% else %}

CREATE TABLE IF NOT EXISTS {{cookiecutter.database_name}}.sample
(
    id      SERIAL PRIMARY KEY,
    name         TEXT,
    description  TEXT NOT NULL,
    created_by   TEXT NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL
);
{% endif %}