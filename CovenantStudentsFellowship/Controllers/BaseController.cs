using CovenantStudentsFellowship.Models;
using Microsoft.AspNetCore.Mvc;

namespace CovenantStudentsFellowship.Controllers
{
    public class BaseController : Controller
    {
        protected void SetMessage(string message, Message.Category messageType)
        {
            Message msg = new Message(message, (int)messageType);
            TempData["Message"] = msg;
        }
    }
}
