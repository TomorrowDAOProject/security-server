namespace SecurityServer.Command;

public interface ICommand
{

    string Name();
    
    void Run();

}