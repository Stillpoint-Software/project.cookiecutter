{% if cookiecutter.include_aspire == "no" %}
    {% include '/templates/docker/api_docker_startup.cs' %}
{% else %}
    {% include '/template/aspire/api_aspire_startup.cs' %}
{% endif %}
