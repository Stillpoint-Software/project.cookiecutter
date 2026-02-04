{%  if cookiecutter.database == 'PostgreSql' %}
{% include 'templates/migration/initial_postgresql.cs' %}
{% elif cookiecutter.database == 'MongoDb' %}
{% include 'templates/migration/initial_mongodb.cs' %}
{%- endif %}