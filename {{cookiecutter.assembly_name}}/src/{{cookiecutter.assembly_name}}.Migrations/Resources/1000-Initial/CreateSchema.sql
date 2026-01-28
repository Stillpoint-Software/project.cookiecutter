CREATE SCHEMA IF NOT EXISTS {{cookiecutter.database_name| lower}};

{% if cookiecutter.include_audit  %}
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TABLE IF NOT EXISTS {{cookiecutter.database_name| lower}}.audit_event
(
	event_id     SERIAL PRIMARY KEY,
	data         jsonb,
	last_updated TIMESTAMP WITH TIME ZONE,
	event_type   TEXT NOT NULL
);

/* PGP: Symmetric */
CREATE OR REPLACE FUNCTION {{cookiecutter.database_name| lower}}.db_sym_encrypt(t text, k text) RETURNS bytea AS $function$
BEGIN
   RETURN pgp_sym_encrypt(t, k);
END;
$function$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION {{cookiecutter.database_name| lower}}.db_sym_decrypt(t bytea, k text) RETURNS text AS $function$
BEGIN
   RETURN pgp_sym_decrypt(t,k);
END;
$function$ LANGUAGE plpgsql;	
{% endif %}

CREATE TABLE IF NOT EXISTS {{cookiecutter.database_name| lower}}.sample
(
    id      SERIAL PRIMARY KEY,
    name         TEXT,
    {% if cookiecutter.include_audit %}
    description   BYTEA NOT NULL,
    {% else %}
    description  TEXT NOT NULL,
    {% endif %}
    created_by   TEXT NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL
);
