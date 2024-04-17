﻿using System.Runtime.CompilerServices;
using FluentValidation;
using FluentValidation.Results;
using {{cookiecutter.assembly_name}}.Api.Validators;
using Hyperbee.Pipeline;
using Hyperbee.Pipeline.Context;
using Microsoft.Extensions.DependencyInjection;

namespace {{cookiecutter.assembly_name}}.Api.Commands.Infrastucture;

public enum ValidationAction
{
    CancelAfter,
    ContinueAfter
}

public static class Validation
{
    public static ValidationFailure Failure( string propertyName, string errorMessage, string errorCode = null ) => new( propertyName, errorMessage ) { ErrorCode = errorCode };
}

public static class PipelineValidationExtensions
{
    private const string ValidationResultKey = nameof( ValidationResultKey );

    // context extensions

    // fail helper

    public static void FailAfter( this IPipelineContext context, string message, int code, [CallerMemberName] string propertyName = default )
    {
        context.SetValidationResult(
            Validation.Failure( propertyName, message, code.ToString() ),
            ValidationAction.CancelAfter );
    }

    public static void FailAfter( this IPipelineContext context, string message, [CallerMemberName] string propertyName = default )
    {
        context.SetValidationResult(
            Validation.Failure( propertyName, message ),
            ValidationAction.CancelAfter );
    }

    // validation result

    public static ValidationResult GetValidationResult( this IPipelineContext context )
        => context.Items.TryGetValue<ValidationResult>( ValidationResultKey, out var item ) ? item : default;

    public static void SetValidationResult( this IPipelineContext context, ValidationResult validationResult, ValidationAction validationAction = ValidationAction.ContinueAfter )
    {
        context.Items.SetValue( ValidationResultKey, validationResult );

        if ( validationAction == ValidationAction.CancelAfter )
            context.CancelAfter();
    }

    public static void SetValidationResult( this IPipelineContext context, List<ValidationFailure> validationFailures, ValidationAction validationAction = ValidationAction.ContinueAfter )
    {
        var validationResult = new ValidationResult( validationFailures );

        context.SetValidationResult( validationResult, validationAction );
    }

    public static void SetValidationResult( this IPipelineContext context, ValidationFailure validationFailure, ValidationAction validationAction = ValidationAction.ContinueAfter )
    {
        context.SetValidationResult( [validationFailure], validationAction );
    }

    public static void AddValidationResult( this IPipelineContext context, ValidationFailure validationFailure, ValidationAction validationAction = ValidationAction.ContinueAfter )
    {
        if ( context.Items.TryGetValue<ValidationResult>( ValidationResultKey, out var validationResult ) )
        {
            validationResult.Errors.Add( validationFailure );
            return;
        }

        context.SetValidationResult( [validationFailure], validationAction );
    }

    public static void ClearValidationResult( this IPipelineContext context )
        => context.Items.SetValue<ValidationResult>( ValidationResultKey, null );

    public static bool IsValid( this IPipelineContext context )
        => context.GetValidationResult()?.IsValid ?? true;

    public static IEnumerable<ValidationFailure> ValidationFailures( this IPipelineContext context )
        => context.GetValidationResult()?.Errors ?? Enumerable.Empty<ValidationFailure>();

    // pipeline builder extensions
    public static IPipelineBuilder<TInput, TOutput> IfValidAsync<TInput, TOutput>(
        this IPipelineBuilder<TInput, TOutput> pipeline,
        IValidator validator,
        Func<IPipelineStartBuilder<TOutput, TOutput>, IPipelineBuilder> builder )
    {
        async Task<TOutput> ValidateAsync( IPipelineContext context, TOutput argument )
        {
            var validationContext = new ValidationContext<object>( argument );
            var result = await validator.ValidateAsync( validationContext );
            context.SetValidationResult( result );

            return argument;
        }

        // usage: .IfValidAsync( Validator.For<ModelType>(), builder => ... )
        return pipeline
            .PipeAsync( ValidateAsync )
            .CallIf( ( c, _ ) => c.IsValid(), builder );
    }

    public static IPipelineBuilder<TInput, TOutput> CancelOnFailure<TInput, TOutput>(
        this IPipelineBuilder<TInput, TOutput> pipeline,
        IValidator validator )
    {
        async Task<TOutput> ValidateAsync( IPipelineContext context, TOutput argument )
        {
            var validationContext = new ValidationContext<object>( argument );
            var result = await validator.ValidateAsync( validationContext );
            context.SetValidationResult( result );

            return argument;
        }

        // usage: .CancelOnFailed( Validator.For<ModelType>() )
        return pipeline
            .PipeAsync( ValidateAsync )
            .Pipe( ( c, a ) =>
            {
                if ( !c.IsValid() )
                    c.CancelAfter();

                return a;
            } );
    }

    public static IPipelineBuilder<TInput, TOutput> CancelOnFailure<TInput, TOutput>(
        this IPipelineBuilder<TInput, TOutput> pipeline,
        Func<IValidatorProvider, IValidator> validatorFunc )
    {
        async Task<TOutput> ValidateAsync( IPipelineContext context, TOutput argument )
        {
            var provider = context.ServiceProvider.GetService<IValidatorProvider>();

            var validator = validatorFunc( provider );
            var validationContext = new ValidationContext<object>( argument );
            var result = await validator.ValidateAsync( validationContext );
            context.SetValidationResult( result );

            return argument;
        }

        // usage: .CancelOnFailed( ( IValidatorProvider provider ) => provider.For<Post>(); )
        return pipeline
            .PipeAsync( ValidateAsync )
            .Pipe( ( c, a ) =>
            {
                if ( !c.IsValid() )
                    c.CancelAfter();

                return a;
            } );
    }

    public static IPipelineBuilder<TInput, TOutput> CancelWithValidationResult<TInput, TOutput>(
        this IPipelineBuilder<TInput, TOutput> pipeline,
        ValidationFailure validationFailure )
    {
        // usage: .CancelWithValidationResult( validationFailure )
        return pipeline
            .Pipe( ( c, a ) =>
            {
                c.SetValidationResult( new List<ValidationFailure> { validationFailure }, ValidationAction.CancelAfter );
                return a;
            } );
    }
}
