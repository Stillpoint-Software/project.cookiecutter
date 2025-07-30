group.MapGet("/{sampleId:int}", async (
          [FromServices] IGetSampleCommand command,
          int sampleId,
          CancellationToken cancellationToken) =>
      {
          var result = await command.ExecuteAsync(sampleId, cancellationToken);
          return result.ToResult();
      });

group.MapPut("/{sampleId:int}", async (
    [FromServices] IUpdateSampleCommand command,
    int sampleId,
    [FromBody] SampleRequest request,
    CancellationToken cancellationToken) =>
{
    var result = await command.ExecuteAsync(request.ToCommand(sampleId), cancellationToken);
    return result.ToResult();
});