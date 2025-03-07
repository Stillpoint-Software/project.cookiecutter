
{% if cookiecutter.use_aspire == "no" %}
 {% include '/templates/docker/migration_docker_program.cs' %}
{% else %}
 {% include '/templates/aspire/migration_aspire_program.cs' %}
{% endif %}
