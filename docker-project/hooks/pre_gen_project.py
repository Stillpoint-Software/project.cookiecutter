import sys
import cookiecutter.prompt

if "{{ cookiecutter.include_oauth }}" == "yes":
    cookiecutter.prompt.read_user_variable("oauth_app_name","Enter the OAuth application name for development, ex:(https://{project_dev_domain}/api/v2/)")
    cookiecutter.prompt.read_user_variable("oauth_audience","Enter the OAuth audience for development, ex:(https://{project_dev_domain}/api/v2/)")
    cookiecutter.prompt.read_user_variable("oauth_api_audience_dev","Enter the OAuth audience for development, ex:(https://{project_dev_domain}/api/v2/)")
    cookiecutter.prompt.read_user_variable("oauth_api_audience_prod","Enter the OAuth audience for production, ex:(https://{project_production_domain}/api/v2/)")
    cookiecutter.prompt.read_user_variable("oauth_domain_dev","Enter the OAuth dev url. ex:(dev-22u8ixu7nxxc581t.us.auth0.com)")
    cookiecutter.prompt.read_user_variable("oauth_domain_prod","Enter the OAuth production url. ex:(main-22u8ixu7nxxc581t.us.auth0.com")
else:
    """{{ cookiecutter.update(
        {
            "oauth_app_name": "",
            "oauth_audience": "",
            "oauth_api_audience_dev": "",
            "oauth_api_audience_prod": "",
            "oauth_domain_dev": "",
            "oauth_domain_prod": "",

        }
    )}}"""

sys.exit(0)