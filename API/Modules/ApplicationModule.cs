﻿using API.Mappings;
using AutoMapper;

namespace API.Modules
{
    public static class ApplicationModule
    {

        public static Profile ApplicationUserMappingProfile()
        {
            return new ApplicationUserMappingProfile();
        }
        public static Profile MessageMappingProfile()
        {
            return new MessageMappingProfile();
        }
    }
}
