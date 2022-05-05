namespace NotesRepository.Data
{
    public class ViewOptionService
    {
        private bool _navBarVisible = true;
        public bool FirstLoad { get; set; } = true;

        public Action? OnChanged { get; set; }

        public bool IsNavigationVisible { get { return _navBarVisible; } }

        //Change state by click on the button
        public void Toggle()
        {
            _navBarVisible = !_navBarVisible;//Change
            if (OnChanged != null) OnChanged();//Callback for reload
        }

        //get additional css class for nav bar div
        public string NavBarClass
        {
            get
            {
                if (FirstLoad)
                {
                    FirstLoad = false;
                    return " ";
                }
                if (_navBarVisible) return "slide-in-left ";
                return "slide-out-left";
            }
        }
    }
}
