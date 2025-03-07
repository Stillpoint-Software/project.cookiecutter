
{% if cookiecutter.use_aspire == "no" %}
 {% include '/templates/docker/migration_docker_main_service.cs' %}
{% else %}
 {% include '/templates/aspire/migration_aspire_main_service.cs' %}
{% endif %}

