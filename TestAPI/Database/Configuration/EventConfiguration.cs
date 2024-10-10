﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestAPI.Models;

namespace TestAPI.Database.Configuration;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.Property(e => e.Title).HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(1_000);
    }
}