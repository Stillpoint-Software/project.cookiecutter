
{% if cookiecutter.use_aspire == "no" %}
 {% include 'src/{{ cookiecutter.assembly_name }}.Migrations/docker_main_service.cs' %}
{% else %}
 {% include 'src/{{ cookiecutter.assembly_name }}.Migrations/aspire_main_service.cs' %}
{% endif %}

