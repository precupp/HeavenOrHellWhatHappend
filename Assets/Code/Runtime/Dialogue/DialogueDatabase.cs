using HeavenOrHell.Dialogue;

namespace HeavenOrHell.Dialogue
{
  /// <summary>
  /// Full English witness dialogue trees from the story document.
  /// </summary>
  public static class DialogueDatabase
  {
    public static DialogueGraph GetGraph(string beatId)
    {
      return beatId switch
      {
        "angel1" => BuildAngel1(),
        "demon1" => BuildDemon1(),
        "angel2" => BuildAngel2(),
        "demon2" => BuildDemon2(),
        _ => null
      };
    }

    private static DialogueGraph BuildAngel1()
    {
      return new DialogueGraph
      {
        witnessId = "angel1",
        startNodeId = "a1_open",
        nodes = new[]
        {
          N("a1_open", "Angel 1", "I was drawn out from heaven and for what purpose?", "a1_detective"),
          N("a1_detective", "Detective", "I'm here on Death's business. Gotta answer some questions for me about that escaped debtor from hell.", "a1_service"),
          N("a1_service", "Angel 1", "Yes, of course. I'm at your service.", "a1_choice_root"),

          Choice("a1_choice_root",
            "a1_know_name", "What do you know about him?",
            "a1_hell_warn", "What do they say about him in hell?",
            "a1_escape", "Let's switch the topic then. How might he have done it? Any ideas?"),

          N("a1_know_name", "Angel 1", "While his name is swallowed by the River Styx in forgetfulness forever, memories that remain of his are of his cat, which he loved dearly.", "a1_know_life_choice"),
          Choice("a1_know_life_choice", "a1_know_life", "What was he like in life?"),

          N("a1_know_life", "Angel 1", "He was by all means a hard working individual, largely undeterred by the sins of life. He worked under a great institution, primarily focused on finance and gave to the needy.", "a1_standup"),
          N("a1_standup", "Detective", "So he was a stand up guy?", "a1_insight"),
          N("a1_insight", "Angel 1", "I have insight only into his memories, which are pure and honest. Of course hell talks as does heaven. But little of that is to be believed. Least of all from hell.", "a1_like_what_choice"),
          Choice("a1_like_what_choice", "a1_like_what", "Like what?"),

          N("a1_like_what", "Angel 1", "Some of my colleagues think he may have had assistance but of course this is only speculation. I certainly do not believe it.", "a1_believe_choice"),
          Choice("a1_believe_choice", "a1_believe", "What do you believe happened?"),

          N("a1_believe", "Angel 1", "Perhaps he was innocent after all. Or perhaps not. He might have chosen the rough path of atonement for himself and succeeded, which is rare but not impossible. Either way that is all I know. Farewell.", null, true),

          N("a1_hell_warn", "Angel 1", "Oh, to inquire about hell is the beginning of the end. My divine oath forbids me from speaking on the matter.", "a1_hell_know_choice"),
          Choice("a1_hell_know_choice", "a1_hell_know", "But you know something?"),

          N("a1_hell_know", "Angel 1", "I must not continue this conversation. I apologize for not being able to provide any more information. Excuse me.", null, true),

          N("a1_escape", "Angel 1", "It is hard to say. Hell is strict in its rules of punishment, heaven in the blessings received. However he achieved it there had to be truth in his claim to heaven. But there's work I must attend to. If you'll excuse me.", null, true)
        }
      };
    }

    private static DialogueGraph BuildDemon1()
    {
      return new DialogueGraph
      {
        witnessId = "demon1",
        startNodeId = "d1_open",
        nodes = new[]
        {
          N("d1_open", "Demon 1", "Mhmmmm, the fresh scent of limbo. And you are our little detective, is that right?", "d1_choice_tone"),
          Choice("d1_choice_tone", "d1_tone_polite", "Please don't refer to me like that. Just answer my questions.", "d1_tone_ok", "Whatever you say."),

          N("d1_tone_polite", "Demon 1", "Whatever you say.", "d1_main_choice"),
          N("d1_tone_ok", "Demon 1", "Whatever you say.", "d1_main_choice"),

          Choice("d1_main_choice",
            "d1_know", "What do you know of the escaped debtor?",
            "d1_took", "What did he take?",
            "d1_help", "Think he might have met someone that could have helped him?",
            "d1_angel", "Doesn't sound like what the angel told me?",
            "d1_punish", "Tell me how the debtor was punished in hell",
            "d1_hungry", "You better answer my question devil …"),

          N("d1_know", "Demon 1", "Well, he did not shy away from expanding his fun that's for sure. You know what I mean. Through various substances and such.", "d1_drug_choice"),
          Choice("d1_drug_choice", "d1_drug", "Was he a drug addict?"),

          N("d1_drug", "Demon 1", "That is a very crude way to put it and by no means correct. He revelled in experience. Any kind of experience and he wanted to feel it in his body completely. In my opinion this obsession is always divine.", "d1_sins_choice"),
          Choice("d1_sins_choice", "d1_sins", "Any other sins I should know about?"),

          N("d1_sins", "Demon 1", "Plenty. He was one of Mammon's favourites. Asmodeus loved watching him play and I for one died for the gossip. He was a poster child for hell, which is why his departure burns all the more. But that is all I can say for now. Goodbye, detective.", null, true),

          N("d1_took", "Demon 1", "This and that. A little bit of everything I would say, though he did have a special love for all things stimulating. He was awake for days at a time, experiencing the joy of life for himself and with others.", "d1_help2_choice"),
          Choice("d1_help2_choice", "d1_help2", "Think he might have met someone that could have helped him?"),

          N("d1_help2", "Demon 1", "Perhaps. So far no debtor has managed to escape hell. Not even with assistance from the other creatures. However, if heaven had interfered? Now that would be an interesting story. But I fear I cannot say much more. I have to attend to my own debtors. Farewell ...", null, true),

          N("d1_help", "Demon 1", "Perhaps. So far no debtor has managed to escape hell. Not even with assistance from the other creatures. However, if heaven had interfered? Now that would be an interesting story. But I fear I cannot say much more. I have to attend to my own debtors. Farewell ...", null, true),

          N("d1_angel", "Demon 1", "So you have talked to angels already? Before me? My goodness, I feel betrayed. I'm sure whatever they had to say was as boring as it was worthless. Don't waste any more of my time.", null, true),

          N("d1_punish", "Demon 1", "That entirely depends on his crimes and I did not personally work with him sadly. But from what I know he was stationed quite far down. Very impressive of course.", "d1_low_choice"),
          Choice("d1_low_choice", "d1_low", "Do you know what brought him down that low?"),

          N("d1_low", "Demon 1", "I can't just give it away. That would destroy your fun imagining all the horrors that fell under his responsibility. I can assure you he deserved it.", "d1_sins2_choice"),
          Choice("d1_sins2_choice", "d1_sins2", "Any other sins I should know about?"),

          N("d1_sins2", "Demon 1", "Plenty. He was one of Mammon's favourites. Asmodeus loved watching him play and I for one died for the gossip. He was a poster child for hell, which is why his departure burns all the more. But that is all I can say for now. Goodbye, detective.", null, true),

          N("d1_hungry", "Demon 1", "So hungry for the details? Well, I cannot blame you. Our debtor was a scammer and a fraud. Very naughty. Associated with all kinds of people. If you know what I mean ... But I fear I must fly. Have a wonderful day detective.", null, true)
        }
      };
    }

    private static DialogueGraph BuildAngel2()
    {
      return new DialogueGraph
      {
        witnessId = "angel2",
        startNodeId = "a2_open",
        nodes = new[]
        {
          N("a2_open", "Angel 2", "Detective, I have been informed already. What do you want to know?", "a2_root_choice"),
          Choice("a2_root_choice",
            "a2_workplace", "Tell me about the debtor's workplace.",
            "a2_private", "Tell me about the debtor's private life."),

          N("a2_workplace", "Angel 2", "He was working in the financial sector. Worldly, yes but the order therein demands to be respected.", "a2_work_follow_choice"),
          Choice("a2_work_follow_choice",
            "a2_enemies", "Finance is usually a competitive field. Any enemies you know of?",
            "a2_order_doubt", "I doubt that it contains order. Not from what I've seen at least."),

          N("a2_enemies", "Angel 2", "There were people that had ill intent towards him. At the end of his life, miserable as he was, they used him for their benefit, despite a year-long cooperation. Of course they shall rot in hell for their crimes as law oversees it.", "a2_what_done_choice"),
          Choice("a2_what_done_choice",
            "a2_what_done", "What did they do to him?",
            "a2_who", "Who were they?",
            "a2_killed", "They killed him right?",
            "a2_why_dislike", "Why would they dislike him?"),

          N("a2_what_done", "Angel 2", "As he was trying to rid himself of his old worldly habits they slipped into his drink a potent substance which forever knocked the breath from his lungs. He passed the same day over to hell.", "a2_just_choice"),
          Choice("a2_just_choice", "a2_just", "Do you think it was just?"),

          N("a2_just", "Angel 2", "Nothing is just in the world of mortals. Only the court of heaven rights this wrong in the beyond. That is all, detective. Farewell.", null, true),
          N("a2_innocent", "Angel 2", "In part, definitely. It is the unstained part of the soul that remains this way forever. Despite his human flaws he did not deserve it. That is all, detective. Farewell.", null, true),
          N("a2_who", "Angel 2", "Other men like him working in the same sector. They found a way to exploit him and they were afraid as they all are.", "a2_killed2_choice"),
          Choice("a2_killed2_choice", "a2_killed2", "They killed him right?"),
          N("a2_killed2", "Angel 2", "No. They did not end his life. No hand was raised against him. He drank from the cup, and then it was empty and so was his life. That is all I can say. Farewell.", null, true),
          N("a2_killed", "Angel 2", "No. They did not end his life. No hand was raised against him. He drank from the cup, and then it was empty and so was his life. That is all I can say. Farewell.", null, true),
          N("a2_why_dislike", "Angel 2", "There were things the debtor knew that should never have passed into his mind. For this knowledge he paid the price. That is all I can say. Farewell.", null, true),

          N("a2_order_doubt", "Angel 2", "Hell intermingles with heaven but law triumphs. It always does and it always will, despite the eternal struggle.", "a2_guess_choice"),
          Choice("a2_guess_choice", "a2_guess", "I guess."),
          N("a2_guess", "Angel 2", "Any other questions?", "a2_enemies2_choice"),
          Choice("a2_enemies2_choice", "a2_enemies2", "Did the debtor have any enemies?"),
          N("a2_enemies2", "Angel 2", "There were people that had ill intent towards him. At the end of his life, miserable as he was, they used him for their benefit, despite a year-long cooperation. Of course they shall rot in hell for their crimes as law oversees it.", "a2_disagree_choice"),
          Choice("a2_disagree_choice",
            "a2_disagree", "Not sure I agree with that.",
            "a2_innocent2", "Do you think he was innocent?"),
          N("a2_disagree", "Angel 2", "Either way this is how it goes. Farewell.", null, true),
          N("a2_innocent2", "Angel 2", "In part, definitely. It is the unstained part of the soul that remains this way forever. Despite his human flaws he did not deserve it. That is all, detective. Farewell.", null, true),

          N("a2_private", "Angel 2", "He lived a quiet life, alone besides his pet companion. His passion was for the betterment of society, since his upbringing had initially pushed him in a different direction.", "a2_upbringing_choice"),
          Choice("a2_upbringing_choice", "a2_upbringing", "Upbringing?"),
          N("a2_upbringing", "Angel 2", "Yes, he was born into wealth and had never faced suffering. Both his parents provided for him. But he learnt love from the animals around him. Truth however was barred from him as true understanding always comes from suffering in one form or another.", "a2_dark_choice"),
          Choice("a2_dark_choice", "a2_dark", "That's a very dark way of looking at life for an angel."),
          N("a2_dark", "Angel 2", "Without darkness no light shall shine as bright. We need it to lead us into the light and into truth.", "a2_parents_choice"),
          Choice("a2_parents_choice", "a2_parents", "Don't you think he suffered with absent parents?"),
          N("a2_parents", "Angel 2", "An absence of love could be suffering. But there's love to be found everywhere. Perhaps more meaningful than that which we cannot have. And his companions were other creatures, therefore he was not alone. He was loved.", "a2_friends_choice"),
          Choice("a2_friends_choice",
            "a2_friends", "That's one way to look at having no friends.",
            "a2_abstract", "That's getting a bit too abstract for me."),
          N("a2_friends", "Angel 2", "He was appreciated. If your love does not reach to understand this, you still have a long way to go until you reach heaven yourself. Farewell.", null, true),
          N("a2_abstract", "Angel 2", "Then let us talk of something else. Do you have any other questions?", "a2_work_redirect"),
          N("a2_work_redirect", "Detective", "Tell me about the debtor's workplace.", "a2_workplace"),
          N("a2_betterment", "Angel 2", "He wanted to see true change in the world. All the suffering around him has brought him a revelation. To treat others as one would like to be treated. To care for them as you would like to be cared for. It changed him. But these efforts were not appreciated.", "a2_enemies3"),
          N("a2_enemies3", "Detective", "Did the debtor have any enemies?", "a2_enemies")
        }
      };
    }

    private static DialogueGraph BuildDemon2()
    {
      return new DialogueGraph
      {
        witnessId = "demon2",
        startNodeId = "d2_open",
        nodes = new[]
        {
          N("d2_open", "Demon 2", "I have heard so much about you already. Both heaven and hell are delighted. So ask away. I can barely contain myself.", "d2_root_choice"),
          Choice("d2_root_choice",
            "d2_classified", "I was told the debtor had classified information. Is that true?",
            "d2_die", "How did the debtor die?",
            "d2_jealous", "Why would his colleagues do that?",
            "d2_escape", "What do you think of his ascension?",
            "d2_others", "Do you think others will attempt the escape now?"),

          N("d2_classified", "Demon 2", "Yes, oh he knew plenty. Much more than I can even say.", "d2_how_choice"),
          Choice("d2_how_choice",
            "d2_how_smart", "He found out by asking smart questions.",
            "d2_how_people", "He found out by knowing the right people."),

          N("d2_how_smart", "Demon 2", "In a way, yes. But he did not have to be that smart to figure it out. He was part of the organization itself. Those who exchange money for blood, for services. How else would he know little detective?", "d2_org_choice"),
          N("d2_how_people", "Demon 2", "When does that not serve someone, hmm? I'm sure that helped him, his parents had prepared him for the role after all. The prince, a wealthy stuck up crying child, grown and fostered in an environment fit for hell. His enemies were part of the same group as him. They did not like that he had plans to uncover it, but he was a part of it all the same, enjoying its services before the morale kicked in. I suppose that's enough, detective. Farewell.", null, true),

          N("d2_org_choice", "Detective", "He was part of the organization he wanted to end?", "d2_org_yes"),
          N("d2_org_yes", "Demon 2", "Yes but don't think of him too highly now, will you. He loved his parties, the pain of others for a long and good time. As he should of course, I am not the one to shame someone's special interest ... I'm sure that is enough. Farewell.", null, true),

          N("d2_die", "Demon 2", "Through a variety of treats all which were enhanced to provide him with an unforgettable, intense experience of the senses. All of them, might I add. Hell does not play games halfway.", "d2_assist_choice"),
          Choice("d2_assist_choice", "d2_assist", "So someone from hell assisted his death?"),
          N("d2_assist", "Demon 2", "\"Assisted\" ... yes. That is a wonderful way to put it. So delicate. He enjoyed it too you know.", "d2_name_choice"),
          Choice("d2_name_choice",
            "d2_name", "Who was responsible? Give me a name.",
            "d2_insane", "That sounds insane."),
          N("d2_name", "Demon 2", "We all are little responsible. You are too, as the agent of Death. It is inescapable. And I cannot give you a name since there is not just one person to blame.", "d2_understand_choice"),
          Choice("d2_understand_choice",
            "d2_understand", "I don't understand.",
            "d2_bored", "That sounds insane."),
          N("d2_understand", "Demon 2", "I tire of your statements. Ask me something. Anything to stop this encroaching boredom.", "d2_escape2_choice"),
          N("d2_insane", "Demon 2", "I tire of your statements. Ask me something. Anything to stop this encroaching boredom.", "d2_escape2_choice"),
          Choice("d2_escape2_choice", "d2_escape2", "What do you think of his ascension?"),
          N("d2_escape2", "Demon 2", "I think he is foolish to leave behind a sinners paradise. But if he enjoys harp and flute that much, who am I to tell him no. Anything else still?", "d2_possible_choice"),
          Choice("d2_possible_choice", "d2_possible", "Is it even possible to get from hell to heaven?"),
          N("d2_possible", "Demon 2", "Haha, no. It is a beautiful illusion created by the father himself to keep the debtors hopeful, but no one has ever made it, except for our special debtor of course. Though he can still be reeled back as well.", "d2_unsettle_choice"),
          Choice("d2_unsettle_choice", "d2_unsettle", "Does that unsettle you?"),
          N("d2_unsettle", "Demon 2", "Nothing of the sort unsettles me. A debtor is the lowest of the low. Whether they ascend or rot forever is the same to me. Though suffering is more fun personally. But I must get going now. Better conversations await. Farewell!", null, true),

          N("d2_jealous", "Demon 2", "Isn't it obvious? He was way ahead of all of them. But nobody told you that is that right? He lived in a penthouse, above the scum, in a city of glass. Could watch the sun complete its full course. Went to dine in expensive restaurants, wore clothes that cost a lifetime. They were jealous.", "d2_jealous2"),
          N("d2_jealous2", "Detective", "All that just because of jealousy? Hard to believe.", "d2_jealous3"),
          N("d2_jealous3", "Demon 2", "And yet it is so much more likely than you think. Jealousy makes the world go round. It spins it. The spite in everything we do. Some call it passion but its nature delightfully stays the same. It was a pleasure. Farewell.", null, true),

          N("d2_escape", "Demon 2", "I think he is foolish to leave behind a sinners paradise. But if he enjoys harp and flute that much, who am I to tell him no. Anything else still?", "d2_possible2_choice"),
          Choice("d2_possible2_choice", "d2_possible2", "Is it even possible to get from hell to heaven?"),
          N("d2_possible2", "Demon 2", "Haha, no. It is a beautiful illusion created by the father himself to keep the debtors hopeful, but no one has ever made it, except for our special debtor of course. Though he can still be reeled back as well.", "d2_unsettle2_choice"),
          Choice("d2_unsettle2_choice", "d2_unsettle2", "Does that unsettle you?"),
          N("d2_unsettle2", "Demon 2", "Nothing of the sort unsettles me. A debtor is the lowest of the low. Whether they ascend or rot forever is the same to me. Though suffering is more fun personally. But I must get going now. Better conversations await. Farewell!", null, true),

          N("d2_others", "Demon 2", "Possibly? It will certainly be very fun to watch them try. Like moths they shall flock to the flame and be burnt to cinders all the same. Haha.", null, true),
          N("d2_bored", "Demon 2", "I tire of your statements. Ask me something. Anything to stop this encroaching boredom.", "d2_escape2_choice")
        }
      };
    }

    private static DialogueNode N(string id, string speaker, string line, string nextId, bool ends = false)
    {
      return new DialogueNode
      {
        id = id,
        speaker = speaker,
        line = line,
        nextId = nextId,
        endsConversation = ends
      };
    }

    private static DialogueNode Choice(string id, params object[] pairs)
    {
      var options = new DialogueOption[pairs.Length / 2];
      for (var i = 0; i < pairs.Length; i += 2)
        options[i / 2] = new DialogueOption((string)pairs[i + 1], (string)pairs[i]);

      return new DialogueNode { id = id, options = options };
    }
  }
}
