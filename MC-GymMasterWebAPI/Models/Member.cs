using System;
using System.Collections.Generic;

namespace MC_GymMasterWebAPI.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string UserId { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Sex { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Address { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateOnly CreationDate { get; set; }

    public DateOnly ExpirationDate { get; set; }
    public DateOnly LastModifiedDate { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<MemberConnection> MemberConnections { get; set; } = new List<MemberConnection>();

    public virtual ICollection<ShareBoard> ShareBoards { get; set; } = new List<ShareBoard>();

    public virtual ICollection<WorkoutSet> WorkoutSets { get; set; } = new List<WorkoutSet>();
}
