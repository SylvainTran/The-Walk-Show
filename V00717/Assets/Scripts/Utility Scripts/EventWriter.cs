using TMPro;

public class EventWriter
{
    public EventWriter()
    {

    }

    public void AppendText(TMP_Text eventLogText, GameClockEvent e)
    {
        TextMeshProUGUI existingText = eventLogText.GetComponent<TextMeshProUGUI>();
        string appendText = existingText.text;
        appendText += "\n\n" + e.Message;
        existingText.SetText(appendText);
    }

}
