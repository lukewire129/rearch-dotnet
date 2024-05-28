using MauiReactor;
using System;
using Rearch.Reactor.Example.Models;
using Rearch.Reactor.Components;
using Microsoft.Maui.Controls;
using static Rearch.Reactor.Example.Capsules.TodoCapsules;
using System.Threading.Tasks;

namespace Rearch.Reactor.Example.Components;

internal class TodoItem(Todo item) : RearchConsumer
{
    public override VisualNode Render(ICapsuleHandle use)
    {
        var title = item.Title;
        var description = item.Description;
        var id = item.Id;
        var completed = item.Completed;

        var (_, UpdateTodo, DeleteTodo) = use.Invoke(TodoItemsManagerCapsule);

        void Delete() => DeleteTodo(item);
        void ToggleCompletionStatus()
        {
            item.Title = title;
            item.Description = description;
            item.Id = id;
            item.Completed = !completed;

            UpdateTodo(item);
        }

        return Border(
            Grid("27, 27", "Auto, *",
                CheckBox()
                    .IsChecked(item.Completed)
                    .OnCheckedChanged(ToggleCompletionStatus)
                    .GridRowSpan(2),
                Label(item.Title)
                    .FormattedText(() =>
                    {
                        var formattedString = new FormattedString();
                        formattedString.Spans.Add(new Span
                        {
                            Text = item.Title,
                            FontAttributes = FontAttributes.Bold,
                        });
                        return formattedString;
                    })
                    .VCenter()
                    .GridColumn(1),
                Label(item.Description)
                    .VCenter()
                    .GridRow(1)
                    .GridColumn(1)),
            TapGestureRecognizer(ToggleCompletionStatus, 1),
            TapGestureRecognizer(
                async () => await ShowDeletionConfirmationDialogAsync(
                    Delete,
                    this.ContainerPage),
                2)
        );
    }

    private static async Task ShowDeletionConfirmationDialogAsync(
        Action delete,
        MauiControls.Page? containerPage)
    {
        if (containerPage is not null &&
            await containerPage.DisplayAlert(
                "Delete Todo",
                "Are you sure you want to delete this todo?",
                "Delete",
                "Cancel"))
        {
            delete();
        }
    }
}
