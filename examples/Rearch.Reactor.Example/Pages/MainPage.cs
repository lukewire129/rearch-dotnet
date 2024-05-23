﻿using MauiReactor;
using Rearch.Reactor.Components;
using Rearch.Reactor.Example.Components;

namespace Rearch.Reactor.Example.Pages;

partial class MainPage : RearchConsumer
{
    public override VisualNode Render(ICapsuleHandle use)
    {
        return new GlobalWarmUps(new Body());
    }
}
