using MarsFPSKit;
using Mirror;

public static class Kit_NetworkReaderWriter
{
    public static void WritePlayer(this NetworkWriter writer, Kit_Player player)
    {
        writer.WriteBool(player.isBot);
        writer.WriteUInt(player.id);
        writer.WriteSByte(player.team);
        writer.WriteString(player.name);
        writer.WriteUShort(player.kills);
        writer.WriteUShort(player.assists);
        writer.WriteUShort(player.deaths);
        writer.WriteUShort(player.ping);
    }

    public static Kit_Player ReadPlayer(this NetworkReader reader)
    {
        Kit_Player player = new Kit_Player();
        player.isBot = reader.ReadBool();
        player.id = reader.ReadUInt();
        player.team = reader.ReadSByte();
        player.name = reader.ReadString();
        player.kills = reader.ReadUShort();
        player.assists = reader.ReadUShort();
        player.deaths = reader.ReadUShort();
        player.ping = reader.ReadUShort();

        return player;
    }

    public static void WriteBot(this NetworkWriter writer, Kit_Bot bot)
    {
        writer.WriteUInt(bot.id);
        writer.WriteString(bot.name);
        writer.WriteSByte(bot.team);
        writer.WriteUShort(bot.kills);
        writer.WriteUShort(bot.assists);
        writer.WriteUShort(bot.deaths);
    }

    public static Kit_Bot ReadBot(this NetworkReader reader)
    {
        Kit_Bot bot = new Kit_Bot();

        bot.id = reader.ReadUInt();
        bot.name = reader.ReadString();
        bot.team = reader.ReadSByte();
        bot.kills = reader.ReadUShort();
        bot.assists = reader.ReadUShort();
        bot.deaths = reader.ReadUShort();

        return bot;
    }
}