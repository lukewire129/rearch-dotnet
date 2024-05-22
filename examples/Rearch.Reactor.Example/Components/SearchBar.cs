using MauiReactor;
using System;
using Rearch.Reactor.Components;
using static Rearch.Reactor.Example.Capsules.TodoCapsules;

namespace Rearch.Reactor.Example.Components;

internal class SearchBar(double height, Action close) : CapsuleConsumer
{
    public override VisualNode Render(ICapsuleHandle use)
    {
        var (
            filter,
            setQueryString,
            _) =
            use.Invoke(FilterCapsule);
        var query = filter.Query;

        return
            Grid("*", "*,Auto",
                Entry()
                    .Text(query)
                    .OnTextChanged(setQueryString),
                Button("Cancel")
                    .GridColumn(1)
                    .OnClicked(() =>
                    {
                        if (string.IsNullOrEmpty(query))
                        {
                            close();
                        }
                        else
                        {
                            setQueryString(string.Empty);
                        }
                    })
            )
            .HeightRequest(height);
    }
}
