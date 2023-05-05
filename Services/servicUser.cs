
class servicUser
{
   public List<User> users = new List<User>();
    public User this[long chatId]
    {
        get
        {
            return users.FirstOrDefault(u=>u.ChatId==chatId)!;
        }
    }
    public servicUser()
    {
        User user = new User();
    }

   public User CheckUser(long chatId)
    {
        User user;
        if ((users!.Any(u => u.ChatId == chatId)))
        {
            user = users.First(u => u.ChatId == chatId);
        }
        else
        {
            user = new User();
            user.ChatId = chatId;
            users.Add(user);
        }
        return user;
    }

    public void Save()
    {

    }

    public void Read()
    {

    }

}