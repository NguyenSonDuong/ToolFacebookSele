using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class Groups
    {
        private string id;
        private string name;
        private string link;
        private string avatar;
        private string quantity_member;

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Link { get => link; set => link = value; }
        public string Avatar { get => avatar; set => avatar = value; }
        public string Quantity_member { get => quantity_member; set => quantity_member = value; }
    }
}
