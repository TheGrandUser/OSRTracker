using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using OSRTracker;

namespace WinUIEx.Generators.Test;

public class RowClass : NotifyObject
{
   public required string Name { get; set => SetProperty(ref field, value); }
   public int SomeValue { get; set => SetProperty(ref field, value); }

}

[GenerateRowWrapper]
public partial class RowClassWrapper : RowWrapperBase<RowClass>
{
   public RowClassWrapper()
      : base(null)
   {
   }

   public RowClassWrapper(RowClass item)
      : base(item)
   {
      this.AllPropertiesChanged();
   }


   protected override RowClass Create()
   {
      return new RowClass() { Name = "" };
   }
}
