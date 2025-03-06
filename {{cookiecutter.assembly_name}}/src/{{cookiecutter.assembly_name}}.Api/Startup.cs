

{% if cookiecutter.use_aspire == "no" %}
 {% include 'src/{{ cookiecutter.assembly_name }}/docker_program.cs' %}
{% else %}
 {% include 'src/{{ cookiecutter.assembly_name }}/aspire_program.cs' %}
{% endif %}
