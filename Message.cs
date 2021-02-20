namespace KrypteringProg2
{
    class Message
    {
        string user;
        string message;
        int id;

        public Message(string user, string message, int id){
            this.user = user;
            this.message = message;
            this.id = id;
        }

        public string User{
            get{
                return this.user;
            }
        }

        public string UserName{
            get{
                return this.user.Substring(user.IndexOf(' ') + 1);
            }
        }

        public string Text{
            get{
                return this.message;
            }
        }

        public int Id{
            get{
                return this.id;
            }
        }
    }
}