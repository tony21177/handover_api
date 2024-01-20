using AutoMapper;
using handover_api.Models;

namespace handover_api.Service
{
    public class MemberService
    {
        private readonly HandoverContext _dbContext;
        private readonly IMapper _mapper;

        public MemberService(HandoverContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Member? GetMemberByUserId(string userId)
        {
            return _dbContext.Members.Where(member => member.UserId == userId).FirstOrDefault();
        }

        public List<Member> GetActiveMembersByUserIds(List<string> userIdList)
        {
            if (userIdList == null || userIdList.Count == 0)
            {
                return new List<Member>();
            }

            return _dbContext.Members.Where(member => userIdList.Contains(member.UserId) && member.IsActive == true).ToList();
        }

        public Member? GetMemberByAccount(string account)
        {
            var member = _dbContext.Members.Where(member => member.Account == account).FirstOrDefault();
            return member;
        }

        public List<Member> GetAllMembers()
        {
            return _dbContext.Members.ToList();
        }

        public Member? UpdateMember(Member member)
        {
            var toBeUpdateMember = _dbContext.Members.FirstOrDefault(_member => _member.UserId == member.UserId);
            if (toBeUpdateMember == null) return null;
            _mapper.Map(member, toBeUpdateMember);

            // 將變更保存到資料庫
            _dbContext.SaveChanges();
            return member;
        }

        public Member CreateMember(Member newMember)
        {
            _dbContext.Members.Add(newMember);
            _dbContext.SaveChanges(true);
            return newMember;
        }

        public void DeleteMember(string userId)
        {
            var membersToDelete = _dbContext.Members.Where(member => member.UserId == userId).ToList();

            if (membersToDelete.Any())
            {
                _dbContext.Members.RemoveRange(membersToDelete);
                _dbContext.SaveChanges();
            }
        }
    }
}
