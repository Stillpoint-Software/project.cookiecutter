  var sample = await _sampleService.GetSampleAsync( sampleId );

        if ( sample == null )
        {
            context.AddValidationResult( new ValidationFailure( nameof( sample ), "Sample does not exist" ) );
            context.CancelAfter();
            return null;
        }
        using var auditScope = AuditScope.Create( "Sample:Read", () => sample );       

        return sample;