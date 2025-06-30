{%- if cookiecutter.database == 'PostgreSql' %}
{% include '/templates/migration/migration_initial_postgresql.cs' %}
{% elif cookiecutter.database == 'MongoDb' %}
{% include '/templates/migration/migration_initial_mongodb.cs' %}
{%- endif %}