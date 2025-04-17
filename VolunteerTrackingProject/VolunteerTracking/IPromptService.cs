namespace VolunteerTracking;

public interface IPromptService
{
    string AskUserType();
    bool ConfirmRegistration();
    string AskUsername();
    string AskNewPassword();
    string AskPasswordConfirmation();
    string PromptForInput(string message);
    void WaitForUserAcknowledgement(string message);
}
