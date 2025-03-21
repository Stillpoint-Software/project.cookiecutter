
{% if cookiecutter.include_aspire == "no" %}
 {% include '/templates/docker/migration/migration_main_service.cs' %}
{% else %}
 {% include '/templates/aspire/migration/migration_main_service.cs' %}
{% endif %}

