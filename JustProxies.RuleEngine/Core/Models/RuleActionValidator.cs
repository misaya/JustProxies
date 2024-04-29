using FluentValidation;

namespace JustProxies.RuleEngine.Core.Models;

public class RuleActionValidator : AbstractValidator<RuleAction>
{
    public RuleActionValidator()
    {
        RuleFor((x => x.Resource))
            .Must(p => p.ContainsKey("url"))
            .When(x => x.ActionType == RuleActionType.UrlReWrite)
            .WithMessage("url must be set.");


        RuleFor((x => x.Resource))
            .Must(p => p.ContainsKey("body"))
            .When(x => x.ActionType == RuleActionType.CustomizeResponseContent)
            .WithMessage("body must be set.");
    }
}