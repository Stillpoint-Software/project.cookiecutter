{% if cookiecutter.include_aspire == "no" %}
{% include '/templates/docker/api/api_startup.cs' %}
{% else %}
{% include '/templates/aspire/api/api_startup.cs' %}
{% endif %}
