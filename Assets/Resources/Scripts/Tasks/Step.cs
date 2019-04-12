using System;

public class Step {
    public Type commandType;
    public ICommandArgs args;
    public bool done = false;

    public Step(Type commandType, ICommandArgs args) {
        this.args = args;
        this.commandType = commandType;
    }
}
