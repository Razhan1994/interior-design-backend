namespace InteriorMarketplace.Modules.VendorOffers.Domain;
public readonly record struct OfferId(Guid Value) { public static OfferId New()=>new(Guid.NewGuid()); }
public enum OfferStatus { Pending, Accepted, Rejected, Withdrawn }
public sealed class Offer
{
    private Offer() { }
    public Offer(OfferId id,Guid projectId,Guid elementId,Guid vendorId,decimal price,int deliveryDays,string? note,string? productImageUrl,DateTime createdAtUtc)
    { Id=id;ProjectId=projectId;ElementId=elementId;VendorId=vendorId;CreatedAtUtc=createdAtUtc;Update(price,deliveryDays,note,productImageUrl); }
    public OfferId Id{get;private set;} public Guid ProjectId{get;private set;} public Guid ElementId{get;private set;} public Guid VendorId{get;private set;}
    public decimal Price{get;private set;} public int DeliveryDays{get;private set;} public string? Note{get;private set;} public string? ProductImageUrl{get;private set;}
    public OfferStatus Status{get;private set;} public DateTime CreatedAtUtc{get;private set;}
    public void Update(decimal price,int deliveryDays,string? note,string? imageUrl){EnsurePending();if(price<=0)throw new ArgumentOutOfRangeException(nameof(price));if(deliveryDays<=0)throw new ArgumentOutOfRangeException(nameof(deliveryDays));Price=price;DeliveryDays=deliveryDays;Note=note;ProductImageUrl=imageUrl;}
    public void Withdraw(){EnsurePending();Status=OfferStatus.Withdrawn;} public void Accept(){EnsurePending();Status=OfferStatus.Accepted;} public void Reject(){if(Status==OfferStatus.Pending)Status=OfferStatus.Rejected;}
    private void EnsurePending(){if(Status!=OfferStatus.Pending)throw new InvalidOperationException("Only pending offers may be changed.");}
}
