namespace HeavenOrHell.UI
{
  /// <summary>
  /// Story-Texte für Intro, Beat-Ziele, Beschwörungen und Finale.
  /// </summary>
  public static class StoryNarrativeContent
  {
    public const string Intro =
      "You are a freelance detective. Death has summoned you to an impossible case:\n" +
      "a soul has ascended from Hell to Heaven — a path that should never exist.\n\n" +
      "You stand in Limbo: a neutral office tower where Heaven and Hell may meet " +
      "without tearing reality apart. Angels and demons cannot simply walk in to testify.\n\n" +
      "Channel their energy through the cauldron — four offerings at a time — " +
      "and summon them one by one. Uncover the truth before Death demands your verdict.";

    public static string GetBeatObjective(string beatId, string chapterId)
    {
      var energy = chapterId == "hell" ? "Hell" : "Heaven";
      return beatId switch
      {
        "angel1" => $"Gather {energy} energy scattered through the office.\nThrow 4 pieces into the cauldron to summon Angel 1.",
        "demon1" => $"Collect {energy} energy from the marked objects.\nOffer 4 to the cauldron to draw Demon 1 into Limbo.",
        "angel2" => $"More {energy} energy is needed.\nFill the cauldron with 4 offerings to summon Angel 2.",
        "demon2" => $"One last round of {energy} energy.\nFour offerings will bring Demon 2 to your office.",
        _ => $"Collect {energy} energy and throw 4 items into the cauldron."
      };
    }

    public static string GetSummonText(string beatId)
    {
      return beatId switch
      {
        "angel1" => "The cauldron hums with celestial light.\nAngel 1 takes form in your office, ready to speak.",
        "demon1" => "Infernal smoke coils from the cauldron.\nDemon 1 grins as they materialize before you.",
        "angel2" => "A second radiance spills across the room.\nAngel 2 kneels, prepared to answer your questions.",
        "demon2" => "The office trembles with heat.\nDemon 2 arrives, delighted by the chaos you have stirred.",
        _ => "Enough energy has been gathered. A witness appears."
      };
    }

    public const string FinalePrompt =
      "You have heard from both sides. The escaped debtor's fate rests in your hands.\n\n" +
      "Choose your verdict.";

    public const string EndingHeaven =
      "From the evidence you have gathered it seems as if neither party is completely right nor wrong. " +
      "The debtor was part of a terrible organization and did despicable things, but he also chose to atone for his sins. " +
      "How can you keep someone from trying to better themselves?\n\n" +
      "As you decide to let him stay in heaven a small spark of light dances in front of you, " +
      "before shooting upwards into the darkness, where it would remain amongst the stars.";

    public const string EndingHell =
      "Despite the evidence you have gathered, this debtor doesn't seem right. " +
      "How could he have lived in sin for so long at the expense of others without feeling a tinge of remorse? " +
      "Perhaps atonement is limited. It has no space for those claiming it when it suits them.\n\n" +
      "As you damn the debtor to hell you hear bells chiming around you. The sound of eternal damnation. " +
      "A spark is turned into a bright flame which is hurled into a chasm before you, slowly fading as it's swallowed by darkness.";

    public const string EndingCondemn =
      "You don't get the full picture. Perhaps this debtor was a good person or perhaps not. There's no way of telling. " +
      "And Death demands an answer. You decide to condemn the debtor. Better to keep him out than let him stay — " +
      "but as you utter your sentence a cold wind grips you and tears you down through the floors into a magma tar pit trap. " +
      "The debtor you have condemned screams at you as you both melt away into nothing.";
  }
}
