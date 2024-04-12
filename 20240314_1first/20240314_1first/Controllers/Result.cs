namespace _20240314_1first.Controllers
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Value { get; set; }
        public static Result SwitchCaseToSuccess(string type )
        {
            switch (type)
            {
                case "POST":
                    return new Result { IsSuccess = true, Message = "Create Successful" };
                case "PUT":
                    return new Result { IsSuccess = true, Message = "Update Successful" };
                case "DELETE":
                    return new Result { IsSuccess = true, Message = "Delete Successful" };
            }
            return new Result { IsSuccess = false, Message = "Something went wrong" };
        }

        public static Result ToFailed(string type)
        {
            switch (type)
            {
                case "POST":
                    return new Result { IsSuccess = false, Message = "Create Not Successful" };
                case "PUT":
                    return new Result { IsSuccess = false, Message = "Update Not Successful" };
                case "DELETE":
                    return new Result { IsSuccess = false, Message = "Delete Not Successful" };
            }
            return new Result { IsSuccess = false, Message = "Something went wrong" };
        }
        //public static Result Exist(bool IsSuccess, string value)
        //{
        //    if (IsSuccess)
        //    {
        //        return new Result { IsSuccess = true, Value = $"{value} is exist." };
        //    }
        //    return new Result { IsSuccess = false, Value = $"{value} does not exist." };
        //}
    }
}
