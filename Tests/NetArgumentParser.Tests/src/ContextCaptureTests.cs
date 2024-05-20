using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Tests;

public class ContextCaptureTests
{
    [Fact]
    public void GetNumberOfItemsToCapture_EmptyContextCapture_CaptureCorrectly()
    {
        string[] context1 = [];
        string[] context2 = ["64"];
        string[] context3 = ["-q", "10"];
        string[] context4 = ["Open", "Create", "OpenOrCreate"];
        string[] context5 = ["-19.5", "15", "126.01", "--angle"];

        var contexts = new List<string[]>()
        {
            context1, context2, context3, context4, context5 
        };

        var contextCapure = new EmptyContextCapture();

        foreach (string[] context in contexts)
        {
            int items = contextCapure.GetNumberOfItemsToCapture(context);
            Assert.Equal(0, items);
        }
    }

    [Fact]
    public void GetNumberOfItemsToCapture_FixedContextCapture_CaptureCorrectly()
    {
        string[] context1 = [];
        string[] context2 = ["64"];
        string[] context3 = ["-q", "10"];
        string[] context4 = ["Open", "Create", "OpenOrCreate"];
        string[] context5 = ["-19.5", "15", "126.01", "--angle"];

        var contexts = new List<string[]>()
        {
            context1, context2, context3, context4, context5 
        };

        for (int i = 1; i <= contexts.Max(t => t.Length); ++i)
        {
            var contextCapure = new FixedContextCapture(i);

            foreach (string[] context in contexts)
            {
                if (i > context.Length)
                {
                    Assert.Throws<NotEnoughValuesInContextException>(() =>
                    {
                        int items = contextCapure.GetNumberOfItemsToCapture(context);
                    });
                }
                else
                {
                    int items = contextCapure.GetNumberOfItemsToCapture(context);
                    Assert.Equal(i, items);
                }
            }
        }
    }

    [Fact]
    public void GetNumberOfItemsToCapture_OneOrMoreContextCapture_CaptureCorrectly()
    {
        string[] context1 = [];
        string[] context2 = ["-q", "10"];

        string[] context3 = ["64"];
        string[] context4 = ["Open", "Create", "OpenOrCreate"];
        string[] context5 = ["-19.5", "15", "126.01", "--angle", "14"];

        string[] context6 = ["/L", "15", "126.01", "--angle"];
        string[] context7 = ["-19.5", "15", "126.01", "/a", "10"];

        var contextCapure = new OneOrMoreContextCapture();

        Assert.Throws<NotEnoughValuesInContextException>(() =>
        {
            int items1 = contextCapure.GetNumberOfItemsToCapture(context1);
        });

        Assert.Throws<NotEnoughValuesInContextException>(() =>
        {
            int items2 = contextCapure.GetNumberOfItemsToCapture(context2);
        });

        int items3 = contextCapure.GetNumberOfItemsToCapture(context3);
        int items4 = contextCapure.GetNumberOfItemsToCapture(context4);
        int items5 = contextCapure.GetNumberOfItemsToCapture(context5);
        
        Assert.Equal(1, items3);
        Assert.Equal(3, items4);
        Assert.Equal(3, items5);
        
        int items6SlashOptionDisabled = contextCapure.
            GetNumberOfItemsToCapture(context6, false);

        int items7SlashOptionDisabled = contextCapure
            .GetNumberOfItemsToCapture(context7, false);

        Assert.Equal(3, items6SlashOptionDisabled);
        Assert.Equal(5, items7SlashOptionDisabled);

        Assert.Throws<NotEnoughValuesInContextException>(() =>
        {
            int items6SlashOptionEnabled = contextCapure.
                GetNumberOfItemsToCapture(context6, true);
        });

        int items7SlashOptionEnabled = contextCapure
            .GetNumberOfItemsToCapture(context7, true);

        Assert.Equal(3, items7SlashOptionEnabled);
    }

    [Fact]
    public void GetNumberOfItemsToCapture_ZeroOrMoreContextCapture_CaptureCorrectly()
    {
        string[] context1 = [];
        string[] context2 = ["-q", "10"];

        string[] context3 = ["64"];
        string[] context4 = ["Open", "Create", "OpenOrCreate"];
        string[] context5 = ["-19.5", "15", "126.01", "--angle", "14"];

        string[] context6 = ["/L", "15", "126.01", "--angle"];
        string[] context7 = ["-19.5", "15", "126.01", "/a", "10"];

        var contextCapure = new ZeroOrMoreContextCapture();

        int items1 = contextCapure.GetNumberOfItemsToCapture(context1);
        int items2 = contextCapure.GetNumberOfItemsToCapture(context2);

        Assert.Equal(0, items1);
        Assert.Equal(0, items2);

        int items3 = contextCapure.GetNumberOfItemsToCapture(context3);
        int items4 = contextCapure.GetNumberOfItemsToCapture(context4);
        int items5 = contextCapure.GetNumberOfItemsToCapture(context5);
        
        Assert.Equal(1, items3);
        Assert.Equal(3, items4);
        Assert.Equal(3, items5);
        
        int items6SlashOptionDisabled = contextCapure.
            GetNumberOfItemsToCapture(context6, false);

        int items7SlashOptionDisabled = contextCapure
            .GetNumberOfItemsToCapture(context7, false);

        Assert.Equal(3, items6SlashOptionDisabled);
        Assert.Equal(5, items7SlashOptionDisabled);

        int items6SlashOptionEnabled = contextCapure.
            GetNumberOfItemsToCapture(context6, true);

        int items7SlashOptionEnabled = contextCapure
            .GetNumberOfItemsToCapture(context7, true);

        Assert.Equal(0, items6SlashOptionEnabled);
        Assert.Equal(3, items7SlashOptionEnabled);
    }

    [Fact]
    public void GetNumberOfItemsToCapture_ZeroOrOneContextCapture_CaptureCorrectly()
    {
        string[] context1 = [];
        string[] context2 = ["-q", "10"];

        string[] context3 = ["64"];
        string[] context4 = ["Open", "Create", "OpenOrCreate"];
        string[] context5 = ["-19.5", "15", "126.01", "--angle", "14"];

        string[] context6 = ["/L", "15", "126.01", "--angle"];
        string[] context7 = ["-19.5", "15", "126.01", "/a", "10"];

        var contextCapure = new ZeroOrOneContextCapture();

        int items1 = contextCapure.GetNumberOfItemsToCapture(context1);
        int items2 = contextCapure.GetNumberOfItemsToCapture(context2);

        Assert.Equal(0, items1);
        Assert.Equal(0, items2);

        int items3 = contextCapure.GetNumberOfItemsToCapture(context3);
        int items4 = contextCapure.GetNumberOfItemsToCapture(context4);
        int items5 = contextCapure.GetNumberOfItemsToCapture(context5);
        
        Assert.Equal(1, items3);
        Assert.Equal(1, items4);
        Assert.Equal(1, items5);
        
        int items6SlashOptionDisabled = contextCapure.
            GetNumberOfItemsToCapture(context6, false);

        int items7SlashOptionDisabled = contextCapure
            .GetNumberOfItemsToCapture(context7, false);

        Assert.Equal(1, items6SlashOptionDisabled);
        Assert.Equal(1, items7SlashOptionDisabled);

        int items6SlashOptionEnabled = contextCapure.
            GetNumberOfItemsToCapture(context6, true);

        int items7SlashOptionEnabled = contextCapure
            .GetNumberOfItemsToCapture(context7, true);

        Assert.Equal(0, items6SlashOptionEnabled);
        Assert.Equal(1, items7SlashOptionEnabled);
    }
}
