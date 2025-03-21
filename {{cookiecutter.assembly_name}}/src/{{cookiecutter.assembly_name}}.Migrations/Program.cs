
{% if cookiecutter.include_aspire == "no" %}
{% include '/templates/docker/migration/migration_program.cs' %}
{% else %}
{% include '/templates/aspire/migration/migration_program.cs' %}
{% endif %}
