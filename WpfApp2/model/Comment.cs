using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.model
{
    public class Comment
    {
        private string id;
        private string message;
        private DateTime create_at;
        private string username_comment;
        private string id_user_comment;
        private string link_user_comment;
        private string avatar_user_comment;

        public Comment(string id, string message, DateTime create_at, string username_comment, string id_user_comment, string link_user_comment, string avatar_user_comment)
        {
            this.id = id;
            this.message = message;
            this.create_at = create_at;
            this.username_comment = username_comment;
            this.id_user_comment = id_user_comment;
            this.link_user_comment = link_user_comment;
            this.avatar_user_comment = avatar_user_comment;
        }

        public Comment()
        {
        }

        public string Id { get => id; set => id = value; }
        public string Message { get => message; set => message = value; }
        public DateTime Create_at { get => create_at; set => create_at = value; }
        public string Username_comment { get => username_comment; set => username_comment = value; }
        public string Id_user_comment { get => id_user_comment; set => id_user_comment = value; }
        public string Link_user_comment { get => link_user_comment; set => link_user_comment = value; }
        public string Avatar_user_comment { get => avatar_user_comment; set => avatar_user_comment = value; }
    }
}
