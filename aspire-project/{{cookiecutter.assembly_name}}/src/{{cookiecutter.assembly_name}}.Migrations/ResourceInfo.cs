// declare assembly wide attribute used by resource migrations
// to locate the root resources folder in the assembly manifest

using Hyperbee.Migrations.Resources;

[assembly: ResourceLocation( "{{cookiecutter.assembly_name}}.Migrations.Resources" )]