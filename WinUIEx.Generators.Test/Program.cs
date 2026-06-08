namespace WinUIEx.Generators.Test;

internal class Program
{
   static void Main(string[] args)
   {
      Console.WriteLine("Hello, World!");

      RowClass rc = new RowClass()
      {
         Name = "Test row",
         SomeValue = 17
      };


      RowClassWrapper wrapper = new RowClassWrapper(rc);
      
      Console.WriteLine(wrapper.Name);
   }
}
