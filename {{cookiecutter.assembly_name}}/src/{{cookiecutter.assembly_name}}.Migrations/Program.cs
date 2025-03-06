
{% if cookiecutter.use_aspire == "no" %}
 {% include 'src/{{ cookiecutter.assembly_name }}.Migrations/docker_program.cs' %}
{% else %}
 {% include 'src/{{ cookiecutter.assembly_name }}.Migrations/aspire_program.cs' %}
{% endif %}
