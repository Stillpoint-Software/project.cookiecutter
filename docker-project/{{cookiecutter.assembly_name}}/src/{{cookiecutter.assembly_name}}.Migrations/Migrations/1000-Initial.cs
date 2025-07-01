{% - if cookiecutter.database == 'PostgreSql' %}
{% include '../templates/docker/migration/migration_initial_postgresql.cs' %}
{% elif cookiecutter.database == 'MongoDb' %}
    {% include '../templates/docker/migration/migration_initial_mongodb.cs' %}
    {% -endif %}