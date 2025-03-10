{% if cookiecutter.include_aspire == "no" %}
{% include '/templates/docker/api_docker_startup.cs' %}
{% else %}
{% include '/templates/aspire/api_aspire_startup.cs' %}
{% endif %}
