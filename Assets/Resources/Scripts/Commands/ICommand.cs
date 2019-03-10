
public interface ICommand {

	void Execute(ICommandArgs args);
	void Stop();
	bool isFinished();
}

public interface ICommandArgs {}