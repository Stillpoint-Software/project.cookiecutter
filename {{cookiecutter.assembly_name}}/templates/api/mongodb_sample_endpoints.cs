group.MapGet("/{sampleId}", async (
          [FromServices] IGetSampleCommand command,
          string sampleId,
          CancellationToken cancellationToken) =>
      {
          var result = await command.ExecuteAsync(sampleId, cancellationToken);
          return result.ToResult();
      });

group.MapPut("/{sampleId}", async (
    [FromServices] IUpdateSampleCommand command,
    string sampleId,
    [FromBody] SampleRequest request,
    CancellationToken cancellationToken) =>
{
    var result = await command.ExecuteAsync(request.ToCommand(sampleId), cancellationToken);
    return result.ToResult();
});
